#!/bin/bash 

##################################################
#####             test start                ######
##################################################

test_ethweb3(){
    sendreq "web3_clientVersion" ""
    sendreq "web3_clientVersion" ""

}

test_etheth(){

    sendreq "eth_getBlockByNumber" '"0x100",false'

    var=${height:="`sendreqwithoutformat "eth_blockNumber" '"latest"' | jq -r '.result'`"}
    printf "cur block height: %d\n" $height

    sendreq "eth_getBlockByNumber" '"'$height'",false'
}

test_height() {
    var=${height:="`sendreqwithoutformat "eth_blockNumber" '"latest"' | jq -r '.result'`"}
    printf "cur block height: %d" $height
}