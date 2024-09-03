FROM rabbitmq:3-management

EXPOSE 5672 15672
ENV RABBITMQ_DEFAULT_USER=matheus \
    RABBITMQ_DEFAULT_PASS=1234 \
    RABBITMQ_DEFAULT_VHOST=rabbitmq \
    DISK_FREE_LIMIT.ABSOLUTE=1GB
CMD ["rabbitmq-server"]
#docker run -d -p 5672:5672 -p 15672:15672 --name rabbitmq rabbitmq:latest 