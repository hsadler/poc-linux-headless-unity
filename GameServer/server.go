package main

import (
	"flag"
	"log"
	"net/http"
	"os"

	"github.com/gorilla/websocket"
)

func main() {
	log.SetFlags(log.LstdFlags)
	// handle client connections
	http.HandleFunc("/", func(w http.ResponseWriter, r *http.Request) {
		// upgrade request to websocket and use default options
		upgrader := websocket.Upgrader{}
		ws, err := upgrader.Upgrade(w, r, nil)
		ws.WriteMessage(1, []byte("connection established"))
		if err != nil {
			log.Print("Request upgrade error:", err)
			return
		}
	})
	// run the server
	port := os.Getenv("PORT")
	if port == "" {
		port = "5000"
	}
	addr := flag.String("addr", "0.0.0.0:"+port, "http service address")
	err := http.ListenAndServe(*addr, nil)
	if err != nil {
		log.Fatal("Server start error:", err)
	}
}
