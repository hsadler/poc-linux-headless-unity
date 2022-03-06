package main

import (
	"bytes"
	"encoding/json"
	"flag"
	"fmt"
	"log"
	"net/http"
	"os"

	"github.com/gorilla/websocket"
)

///////////////// HUB /////////////////

type Hub struct {
	CentralClient         *Client
	PlayerClients         map[*Client]bool
	AddCentralClient      chan *Client
	AddPlayerClient       chan *Client
	RemoveClient          chan *Client
	PlayerClientBroadcast chan []byte
}

func (h *Hub) Run() {
	for {
		select {
		case client := <-h.AddCentralClient:
			fmt.Println("adding central client to hub")
			h.CentralClient = client
		case client := <-h.AddPlayerClient:
			fmt.Println("adding player client to hub")
			h.PlayerClients[client] = true
		case client := <-h.RemoveClient:
			if client == h.CentralClient {
				fmt.Println("removing central client from hub")
				h.CentralClient = nil
			} else {
				fmt.Println("removing player client from hub")
				delete(h.PlayerClients, client)
			}
			client.Cleanup()
		case message := <-h.PlayerClientBroadcast:
			for c := range h.PlayerClients {
				c.Send <- message
			}
		}
	}
}

func NewHub() *Hub {
	return &Hub{
		CentralClient:         nil,
		PlayerClients:         make(map[*Client]bool),
		AddCentralClient:      make(chan *Client),
		AddPlayerClient:       make(chan *Client),
		RemoveClient:          make(chan *Client),
		PlayerClientBroadcast: make(chan []byte),
	}
}

///////////////// CLIENT /////////////////

type Client struct {
	Hub      *Hub
	Ws       *websocket.Conn
	ClientId string
	Send     chan []byte
}

func (cl *Client) RecieveMessages() {
	// initialize client's game state by sending server's entire state
	cl.SendGameState()
	// do player removal from game state and websocket close on disconnect
	defer func() {
		fmt.Println("Client.RecieveMessages() goroutine stopping")
		cl.HandleClientDisconnect(nil)
		cl.Ws.Close()
	}()
	for {
		// read message
		_, message, err := cl.Ws.ReadMessage()
		if err != nil {
			log.Println("read:", err)
			break
		}
		// log message received
		fmt.Println("client message received:")
		ConsoleLogJsonByteArray(message)
		// route message to handler
		messageTypeToHandler := map[string]func([]byte){
			// "CLIENT_MESSAGE_TYPE_PLAYER_ENTER":  cl.HandlePlayerEnter,
			// "CLIENT_MESSAGE_TYPE_PLAYER_UPDATE": cl.HandlePlayerUpdate,
			// "CLIENT_MESSAGE_TYPE_PLAYER_EXIT":   cl.HandlePlayerExit,
			// "MESSAGE_TYPE_PLAYER_CLIENT_CONNECT":    cl.HandlePlayerClientConnect,
			// "MESSAGE_TYPE_PLAYER_CLIENT_DISCONNECT": cl.HandlePlayerClientDisconnect,
		}
		var mData map[string]interface{}
		if err := json.Unmarshal(message, &mData); err != nil {
			panic(err)
		}
		// process message with handler
		messageTypeToHandler[mData["messageType"].(string)](message)
	}
}

func (cl *Client) SendMessages() {
	defer func() {
		fmt.Println("Client.SendMessages() goroutine stopping")
	}()
	for message := range cl.Send {
		SendJsonMessage(cl.Ws, message)
	}
}

func (cl *Client) HandleClientDisconnect(m []byte) {
	cl.Hub.RemoveClient <- cl
}

// EXAMPLES OF PRIOR HANDLERS

// func (cl *Client) HandlePlayerEnter(mData map[string]interface{}) {
// 	player := NewPlayerFromMap(mData["player"].(map[string]interface{}), cl.Ws)
// 	cl.Player = player
// 	message := PlayerMessage{
// 		MessageType: "SERVER_MESSAGE_TYPE_PLAYER_ENTER",
// 		Player:      player,
// 	}
// 	serialized, _ := json.Marshal(message)
// 	cl.Hub.Broadcast <- serialized
// }

// func (cl *Client) HandlePlayerUpdate(mData map[string]interface{}) {
// 	player := NewPlayerFromMap(mData["player"].(map[string]interface{}), cl.Ws)
// 	cl.Player = player
// 	message := PlayerMessage{
// 		MessageType: "SERVER_MESSAGE_TYPE_PLAYER_UPDATE",
// 		Player:      player,
// 	}
// 	serialized, _ := json.Marshal(message)
// 	cl.Hub.Broadcast <- serialized
// }

// func (cl *Client) HandlePlayerExit(mData map[string]interface{}) {
// 	message := PlayerMessage{
// 		MessageType: "SERVER_MESSAGE_TYPE_PLAYER_EXIT",
// 		Player:      cl.Player,
// 	}
// 	serialized, _ := json.Marshal(message)
// 	cl.Hub.Broadcast <- serialized
// 	cl.Hub.Remove <- cl
// }

func (cl *Client) SendGameState() {
	allPlayers := make([]*Player, 0)
	// TODO: refactor for new player registry structure when it's ready
	// for client := range cl.Hub.PlayerClients {
	// 	if client.Player != nil {
	// 		allPlayers = append(allPlayers, client.Player)
	// 	}
	// }
	messageData := GameStateMessage{
		MessageType: "SERVER_MESSAGE_TYPE_GAME_STATE",
		GameState: &GameStateJsonSerializable{
			Players: allPlayers,
		},
	}
	serialized, _ := json.Marshal(messageData)
	cl.Send <- serialized
}

func (cl *Client) Cleanup() {
	close(cl.Send)
}

///////////////// GAME ENTITIES /////////////////

type Position struct {
	X float64 `json:"x"`
	Y float64 `json:"y"`
}

type GameBall struct {
	Id       string    `json:"id"`
	Position *Position `json:"position"`
}

func NewGameBallFromMap(pData map[string]interface{}, ws *websocket.Conn) *Player {
	posMap := pData["position"].(map[string]interface{})
	pos := Position{
		X: posMap["x"].(float64),
		Y: posMap["y"].(float64),
	}
	player := Player{
		Id:       pData["id"].(string),
		Position: &pos,
	}
	return &player
}

type Player struct {
	Id       string    `json:"id"`
	Position *Position `json:"position"`
}

func NewPlayerFromMap(pData map[string]interface{}, ws *websocket.Conn) *Player {
	posMap := pData["position"].(map[string]interface{})
	pos := Position{
		X: posMap["x"].(float64),
		Y: posMap["y"].(float64),
	}
	player := Player{
		Id:       pData["id"].(string),
		Position: &pos,
	}
	return &player
}

///////////////// SERVER MESSAGE SENDING /////////////////

func SendJsonMessage(ws *websocket.Conn, messageJson []byte) {
	ws.WriteMessage(1, messageJson)
	// log that message was sent
	// fmt.Println("server message sent:")
	ConsoleLogJsonByteArray(messageJson)
}

type PlayerMessage struct {
	MessageType string  `json:"messageType"`
	Player      *Player `json:"player"`
}

type GameStateJsonSerializable struct {
	Players []*Player `json:"players"`
}

type GameStateMessage struct {
	MessageType string                     `json:"messageType"`
	GameState   *GameStateJsonSerializable `json:"gameState"`
}

///////////////// RUN SERVER /////////////////

func main() {
	flag.Parse()
	log.SetFlags(0)
	// create and run hub singleton
	h := NewHub()
	go h.Run()
	http.HandleFunc("/", func(w http.ResponseWriter, r *http.Request) {
		_, isCentralClient := r.URL.Query()["isCentralClient"]
		// use default options
		upgrader := websocket.Upgrader{}
		ws, err := upgrader.Upgrade(w, r, nil)
		if err != nil {
			log.Print("upgrade:", err)
			return
		}
		// create client, run processes, and add to hub
		cl := &Client{
			Hub:  h,
			Ws:   ws,
			Send: make(chan []byte, 256),
		}
		go cl.RecieveMessages()
		go cl.SendMessages()
		if isCentralClient {
			h.AddCentralClient <- cl
		} else {
			h.AddPlayerClient <- cl
		}
	})
	addr := flag.String("addr", "0.0.0.0:5000", "http service address")
	err := http.ListenAndServe(*addr, nil)
	if err != nil {
		log.Fatal("ListenAndServe: ", err)
	}
}

///////////////// HELPERS /////////////////

func ConsoleLogJsonByteArray(message []byte) {
	var out bytes.Buffer
	message = append(message, "\n"...)
	json.Indent(&out, message, "", "  ")
	out.WriteTo(os.Stdout)
}
