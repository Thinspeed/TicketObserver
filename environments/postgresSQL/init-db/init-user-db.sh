#!/bin/bash
set -e

MIGRATION_USER="migrations"
MIGRATION_USER_PASSWORD="1234"

DB_NAME="tickets"

BACKEND_USER="backend"
BACKEND_PASSWORD='backend_password'

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    CREATE ROLE $MIGRATION_USER WITH REPLICATION LOGIN CREATEDB CREATEROLE PASSWORD '$MIGRATION_USER_PASSWORD';
EOSQL

psql -v ON_ERROR_STOP=1 --username "$MIGRATION_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    CREATE DATABASE $DB_NAME;
EOSQL

psql -v ON_ERROR_STOP=1 --username "$MIGRATION_USER" --dbname "$DB_NAME" <<-EOSQL
    CREATE ROLE $BACKEND_USER WITH LOGIN PASSWORD '$BACKEND_PASSWORD';

    GRANT SELECT, INSERT, UPDATE, DELETE
        ON ALL TABLES
        IN SCHEMA public
        TO $BACKEND_USER;

    ALTER DEFAULT PRIVILEGES
        GRANT SELECT, INSERT, UPDATE, DELETE
        ON TABLES
        TO $BACKEND_USER;

    GRANT USAGE, SELECT ON ALL SEQUENCES IN SCHEMA public TO $BACKEND_USER;
    ALTER DEFAULT PRIVILEGES
        GRANT USAGE, SELECT
        ON SEQUENCES
        TO $BACKEND_USER;

    REVOKE GRANT OPTION
        FOR ALL PRIVILEGES
        ON ALL TABLES
        IN SCHEMA public
        FROM $BACKEND_USER;
    ALTER DEFAULT PRIVILEGES
        REVOKE GRANT OPTION
        FOR ALL PRIVILEGES
        ON TABLES
        FROM $BACKEND_USER;
EOSQL

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$DB_NAME" <<-EOSQL
    REVOKE CREATE ON SCHEMA public FROM PUBLIC;
    GRANT CREATE ON SCHEMA public TO $MIGRATION_USER;
EOSQL