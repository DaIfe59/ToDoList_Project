#!/bin/sh
set -eu

: "${NODE_NAME:=Node}"

envsubst '${NODE_NAME}' < /usr/share/nginx/html/index.template.html > /usr/share/nginx/html/index.html

exec nginx -g 'daemon off;'
