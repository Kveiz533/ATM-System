#pragma warning disable SA1649

using FluentMigrator;

namespace BankApi.Infrastructure.Persistence.Migrations;

[Migration(1773240976, "Initial")]
public sealed class Initial : Migration
{
    public override void Up()
    {
        Execute.Sql("""
                    CREATE TABLE accounts (
                        id BIGSERIAL PRIMARY KEY,
                        account_number TEXT NOT NULL UNIQUE,
                        pin_code TEXT NOT NULL,
                        balance DECIMAL NOT NULL DEFAULT 0
                    );
                    
                    CREATE TABLE admin_sessions (
                        id UUID PRIMARY KEY
                    );
                    
                    CREATE TABLE user_sessions (
                        id UUID PRIMARY KEY,
                        account_id BIGSERIAL NOT NULL
                    );

                    CREATE TABLE operation_history (
                        id BIGSERIAL PRIMARY KEY, 
                        account_id BIGSERIAL NOT NULL,
                        time TIMESTAMPTZ NOT NULL,
                        operation_type INTEGER NOT NULL,
                        balance DECIMAL DEFAULT 0
                    )
                    """);
    }

    public override void Down()
    {
        Execute.Sql("""
                    DROP TABLE IF EXISTS operation_history;
                    DROP TABLE IF EXISTS user_sessions;
                    DROP TABLE IF EXISTS admin_sessions;
                    DROP TABLE IF EXISTS accounts;
                    """);
    }
}