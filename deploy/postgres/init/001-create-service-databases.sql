SELECT 'CREATE DATABASE catalogdb' WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'catalogdb')\gexec
SELECT 'CREATE DATABASE cartdb' WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'cartdb')\gexec
SELECT 'CREATE DATABASE orderdb' WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'orderdb')\gexec
SELECT 'CREATE DATABASE warehousedb' WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'warehousedb')\gexec
SELECT 'CREATE DATABASE paymentdb' WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'paymentdb')\gexec
SELECT 'CREATE DATABASE shippingdb' WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'shippingdb')\gexec
SELECT 'CREATE DATABASE userdb' WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'userdb')\gexec
