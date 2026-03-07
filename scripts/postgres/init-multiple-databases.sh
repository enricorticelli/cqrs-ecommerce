#!/bin/sh
set -e

if [ -z "$POSTGRES_MULTIPLE_DATABASES" ]; then
  echo "POSTGRES_MULTIPLE_DATABASES is empty, skipping multi-db bootstrap."
  exit 0
fi

echo "Creating PostgreSQL databases: $POSTGRES_MULTIPLE_DATABASES"

for db in $(echo "$POSTGRES_MULTIPLE_DATABASES" | tr ',' ' '); do
  cleaned_db=$(echo "$db" | tr -d '[:space:]')
  if [ -z "$cleaned_db" ]; then
    continue
  fi

  echo "Ensuring database '$cleaned_db' exists"
  psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname postgres <<-SQL
    SELECT 'CREATE DATABASE $cleaned_db'
    WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = '$cleaned_db')\gexec
SQL
done

echo "PostgreSQL multi-db bootstrap completed."
