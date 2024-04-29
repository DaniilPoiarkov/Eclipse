docker network rm eclipse
docker network create eclipse

# Set-Location ..

#docker build -f Dockerfile . --tag 'eclipse'

# Set-Location ./docker

docker-compose -f docker-compose.yaml up -d
