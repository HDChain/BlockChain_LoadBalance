#!/bin/bash 

source ./testscript/base.sh
source ./testscript/testeth.sh


#set -v

case $1 in
web3)
	test_ethweb3
	;;
eth)
	test_etheth
	;;
h)
	test_height
	;;
*)
	printhelp
	;;
esac