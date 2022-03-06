
build:
	docker build -t poc-linux-headless-unity:dev -f Dockerfile.dev .

run-server:
	docker-compose -f docker-compose.dev.yaml up

run-unity-client:
	docker exec -it poc-linux-headless-unity /bin/bash -c \
	"cd /GameClient && ls && ./headless.x86_64 -batchmode -nographics -logFile"

shell:
	docker exec -it poc-linux-headless-unity /bin/bash