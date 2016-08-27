package main

import (
	"log"

	"github.com/funny/link"
	"github.com/funny/link/codec"
)

type AddReq struct {
	A, B int
}

type AddRsp struct {
	C int
}

func main() {
	json := codec.Json()
	json.Register(AddReq{})
	json.Register(AddRsp{})

	server, err := link.Serve("tcp", "127.0.0.1:3200", json, 0 /* sync send */)
	checkErr(err)
	log.Println("server start...")
	serverLoop(server)
}

func serverLoop(server *link.Server) {
	for {
		session, err := server.Accept()
		checkErr(err)
		go sessionLoop(session)
	}
}

func sessionLoop(session *link.Session) {
	log.Println("new connection...")
	for {
		req, err := session.Receive()
		checkErr(err)
		log.Println("Receive cmd ")
		err = session.Send(&AddRsp{
			req.(*AddReq).A + req.(*AddReq).B,
		})
		log.Println("Send cmd ")
		checkErr(err)
	}
}

func checkErr(err error) {
	if err != nil {
		log.Fatal(err)
	}
}
