#!/usr/bin/env bash
echo '{"values": [{"key": "url","value": "http://localhost:5000"}]}' > env.json
npx newman run https://www.getpostman.com/collections/08bfa7cf05323e4194fd -e ./env.json && rm env.json
