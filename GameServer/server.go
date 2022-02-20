package main

import (
	"flag"
	"fmt"
	"html"
	"log"
	"net/http"
	"os"
)

func main() {
	log.SetFlags(log.LstdFlags)
	http.HandleFunc("/", func(w http.ResponseWriter, r *http.Request) {
		fmt.Fprintf(w, "Hello, %q", html.EscapeString(r.URL.Path))
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
