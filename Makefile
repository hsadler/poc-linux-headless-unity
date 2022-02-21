
build:
	docker build -t poc-linux-headless-unity:dev -f Dockerfile.dev .

up:
	docker-compose -f docker-compose.dev.yaml up

# TODO: this isn't working
run-unity-client:
	docker exec -it poc-linux-headless-unity \
		cd /GameClient/ && echo "spinning up unity client" && \
        ./headless.x86_64 -batchmode -nographics -logFile

shell:
	docker exec -it poc-linux-headless-unity /bin/bash