FROM liquibase/liquibase:latest

USER root
RUN mkdir -p /liquibase/lib && \
    wget -O /liquibase/lib/postgresql.jar https://jdbc.postgresql.org/download/postgresql-42.7.1.jar
USER liquibase

WORKDIR /liquibase/changelog

COPY change-sets/ ./change-sets/
COPY master.xml ./
