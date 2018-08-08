#!/bin/bash 

id=0
getid(){
    return $(($id+1))
} 

set -v

getid
let id=$?
curl -X POST --data '{"jsonrpc":"2.0","method":"web3_clientVersion","params":[],"id":'$id'}' -H "Content-Type: application/json" http://127.0.0.1:50000/ethrpc/1

getid
let id=$?
curl -X POST --data '{"jsonrpc":"2.0","method":"web3_clientVersion","params":[],"id":'$id'}' -H "Content-Type: application/json" http://127.0.0.1:50000/ethrpc/1


