#!/bin/bash
# #set -e 
cosmosHost=$1
cosmosPort=$2

echo "Installing cosmos db emulator ssl certificate at $cosmosHost:$cosmosPort.."

mkdir /usr/share/ca-certificates/cosmos

for i in {1..60};
do
  curl -fsk https://$cosmosHost:$cosmosPort/_explorer/emulator.pem > /usr/share/ca-certificates/cosmos/emulator.crt
  if [ $? -eq 0 ];  then
    echo "CosmosDB emulator is ready"
    break
  else
    echo "Waiting cosmosdb to start..."
    sleep 10
  fi
done

echo "Adding CosmosDB Cert to Trusted Certs..."
chmod o+r /usr/share/ca-certificates/cosmos/emulator.crt
echo "cosmos/emulator.crt" >> /etc/ca-certificates.conf
update-ca-certificates
