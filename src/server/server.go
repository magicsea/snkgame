// server
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
	log.Printf("serverLoop... ")
	serverLoop(server)
	//addr := server.Listener().Addr().String()
	log.Printf("server stop... ")
}

func serverLoop(server *link.Server) {
	for {
		session, err := server.Accept()
		checkErr(err)
		go sessionLoop(session)
	}
}

func sessionLoop(session *link.Session) {
	for {
		req, err := session.Receive()
		reqtype := req.(*AddReq)
		checkErr(err)

		log.Println("recv req:", reqtype.A, reqtype.B)
		err = session.Send(&AddRsp{
			reqtype.A + reqtype.B,
		})
		checkErr(err)
	}
}

func checkErr(err error) {
	if err != nil {
		log.Fatal(err)
	}
}
