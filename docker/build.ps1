docker network rm eclipse
docker network create eclipse

docker-compose -f docker-compose.yaml -f docker-compose.local.yaml up -d
