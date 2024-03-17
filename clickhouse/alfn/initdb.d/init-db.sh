#!/bin/bash
set -e 

clickhouse client -n <<-EOSQL
    CREATE DATABASE IF NOT EXISTS alfn;
    CREATE TABLE IF NOT EXISTS alfn.primes
    (
	generated_time DateTime64(3) NOT NULL,
	published_time DateTime64(3) NOT NULL,
        nickname       String     NOT NULL,
        number         Int32      NOT NULL
    ) ENGINE = Memory();
EOSQL
