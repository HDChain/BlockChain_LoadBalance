#!/bin/bash 

id=0
url="http://127.0.0.1:50000/ethrpc/1"

#update request id
getid(){
    return $(($id+1))
}

#call curl send request
sendreq(){
	getid
	let id=$?
	
#	echo -e "\033[32m"
#	curl -s -w "\n\ntime : "%{time_total}"s\n" -X POST -d '{"jsonrpc":"2.0","method":"'$1'","params":['$2'],"id":'$id'}' -H "Content-Type: application/json" $url 
if [ ! -f "/usr/bin/jq" ]; then
    curl -s -X POST -d '{"jsonrpc":"2.0","method":"'$1'","params":['$2'],"id":'$id'}' -H "Content-Type: application/json" $url
else
    curl -s -X POST -d '{"jsonrpc":"2.0","method":"'$1'","params":['$2'],"id":'$id'}' -H "Content-Type: application/json" $url | jq
fi
#	echo -e "\033[0m"
}

sendreqwithoutformat(){
	getid
	let id=$?
	
#	echo -e "\033[32m"
#	curl -s -w "\n\ntime : "%{time_total}"s\n" -X POST -d '{"jsonrpc":"2.0","method":"'$1'","params":['$2'],"id":'$id'}' -H "Content-Type: application/json" $url 
	curl -s -X POST -d '{"jsonrpc":"2.0","method":"'$1'","params":['$2'],"id":'$id'}' -H "Content-Type: application/json" $url
#	echo -e "\033[0m"
}



printhelp(){

    echo "curl.sh [command]
command list:
1. web3
2. eth
    "

}
