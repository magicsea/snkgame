package main

import (
	"encoding/binary"
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

	flCodec := codec.FixLen(json, 2, binary.LittleEndian, 1024, 1024)

	client, err := link.Connect("tcp", "127.0.0.1:3200", flCodec, 0)
	checkErr(err)
	clientLoop(client)
}

func clientLoop(session *link.Session) {
	for i := 0; i < 10; i++ {
		err := session.Send(&AddReq{
			i, i,
		})
		checkErr(err)
		log.Printf("Send: %d + %d", i, i)

		rsp, err := session.Receive()
		checkErr(err)
		log.Printf("Receive: %d", rsp.(*AddRsp).C)
	}
}

func checkErr(err error) {
	if err != nil {
		log.Fatal(err)
	}
}
