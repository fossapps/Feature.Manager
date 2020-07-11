#!/usr/bin/env bash
mkdir -p Generated
docker run --net=host -it --rm -v $(pwd)/Generated:/app/sdk node bash -c "apt update && apt install libunwind-dev -y && npm i -g autorest && autorest --input-file=http://localhost:5000/swagger/v1/swagger.json --csharp --namespace=FossApps.FeatureManager --override-client-name=FeatureManager --output-folder=/app/sdk --clear-output-folder"
