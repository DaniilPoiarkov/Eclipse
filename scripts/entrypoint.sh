#!/bin/bash
# #set -e 
cosmosHost=$1
cosmosPort=$2

echo 'Extracting exposed by ngrok url..'
sleep 1

tunnel=$(curl -s http://ngrok:4040/api/tunnels/command_line)
self_url=$(echo $tunnel | jq -r '.public_url')

echo 'Extracted url' $self_url

./install-cosmos-certificate.sh $cosmosHost $cosmosPort

echo "Running Eclipse.WebAPI"
dotnet ./Eclipse.WebAPI.dll App:SelfUrl=$self_url
