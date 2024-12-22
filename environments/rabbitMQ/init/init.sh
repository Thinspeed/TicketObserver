#!/bin/bash

RABBITMQ_USER="observer"
RABBITMQ_PASSWORD="1234"
RABBITMQ_HOST="localhost"
RABBITMQ_HTTP_PORT="15672"
RABBITMQ_VHOST="/observer"

MAX_ATTEMPTS=30
SLEEP_INTERVAL=2


attempt=1

echo "Ожидание запуска RabbitMQ..."

while (( attempt <= MAX_ATTEMPTS )); do
    if rabbitmqctl status > /dev/null 2>&1; then
        echo "RabbitMQ успешно запущен!"

        echo "Создание очереди.."
        rabbitmqadmin -u $RABBITMQ_USER -p $RABBITMQ_PASSWORD -V $RABBITMQ_VHOST declare queue name=tickets_stream durable=true auto_delete=false arguments='{"exclusive":false,"x-queue-type":"stream"}'
        
        echo "Создание обменника.."
        rabbitmqadmin -u $RABBITMQ_USER -p $RABBITMQ_PASSWORD -V $RABBITMQ_VHOST declare exchange name=default_exchange type=direct durable=true auto_delete=false internal=false arguments='{}'

        echo "Создание привязки.."
        rabbitmqadmin -u $RABBITMQ_USER -p $RABBITMQ_PASSWORD -V $RABBITMQ_VHOST declare binding source=default_exchange destination=tickets_stream destination_type=queue routing_key=ticket arguments='{}'

        echo "Rabbitmq инициализирован!"
        exit 0
    else
        echo "Попытка $attempt: RabbitMQ пока не запущен. Ожидание $SLEEP_INTERVAL секунд..."
        sleep $SLEEP_INTERVAL
        ((attempt++))
    fi
done

echo "Ошибка: RabbitMQ не запустился после $(($MAX_ATTEMPTS * $SLEEP_INTERVAL)) секунд."
exit 1