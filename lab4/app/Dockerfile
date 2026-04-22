FROM nginx:1.27-alpine

WORKDIR /usr/share/nginx/html

COPY index.template.html /usr/share/nginx/html/index.template.html
COPY start.sh /start.sh

RUN apk add --no-cache gettext \
    && sed -i 's/\r$//' /start.sh \
    && chmod +x /start.sh

CMD ["/start.sh"]
