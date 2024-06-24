CREATE SEQUENCE account_id_seq START 122141315416121;

-- Create the table using the sequence for the Id column
CREATE TABLE "Account" (
    "Id" BIGINT PRIMARY KEY DEFAULT nextval('account_id_seq'),
    "Enabled" BOOLEAN NOT NULL
);

-- Create the AccountBalance table with AccountId as the primary key and foreign key
CREATE TABLE "AccountBalance" (
    "AccountId" BIGINT PRIMARY KEY,
    "Balance" DECIMAL(18, 2) NOT NULL,
    FOREIGN KEY ("AccountId") REFERENCES "Account"("Id")
);

--drop table "WithdrawOutbox" ;
--drop table "WithdrawOutboxStatus" ;

CREATE TABLE "WithdrawOutboxStatus" (
    "Id" INT PRIMARY KEY,
    "Status" VARCHAR(100) NOT NULL
);

insert into "WithdrawOutboxStatus" select 0,'Pending';
insert into "WithdrawOutboxStatus" select 1,'InProgress';
insert into "WithdrawOutboxStatus" select 2,'Completed';



CREATE TABLE "WithdrawOutbox" (
    "Id" SERIAL PRIMARY KEY,
    "TransactionGuid" UUID NOT NULL UNIQUE,
    "AccountId" BIGINT NOT NULL REFERENCES "Account"("Id"),
    "Amount" DECIMAL(18, 2) NOT NULL,
	"CreatedDate" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
	"RunningBalance" DECIMAL(18, 2) NOT NULL,
    "TransactionHistoryStatusId" INT NOT NULL DEFAULT 0 REFERENCES "WithdrawOutboxStatus"("Id"),
    "MessageServiceStatusId" INT NOT NULL DEFAULT 0 REFERENCES "WithdrawOutboxStatus"("Id"),
    "TransactionHistoryStatusChangeDate" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "MessageServiceStatusChangeDate" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE "TransactionHistory"(
	"Id" SERIAL PRIMARY KEY,
    "TransactionGuid" UUID NOT NULL UNIQUE,
	"AccountId" BIGINT NOT NULL REFERENCES "Account"("Id"),
	"Amount" DECIMAL(18, 2) NOT NULL,
	"TransactionDate" TIMESTAMP NOT NULL,
	"RunningBalance" DECIMAL(18, 2) NOT NULL
);
-- PROCEDURE: public.InsertTransactionHistoryFromWithdrawOutbox()

-- DROP PROCEDURE IF EXISTS public."InsertTransactionHistoryFromWithdrawOutbox"();

CREATE OR REPLACE PROCEDURE public."InsertTransactionHistoryFromWithdrawOutbox"(
	OUT result_message text)
LANGUAGE 'plpgsql'
AS $BODY$
DECLARE
    id BIGINT;
    transaction_guid UUID;
    account_id BIGINT;
    amount DECIMAL(18, 2);
    created_date TIMESTAMP;
    running_balance DECIMAL(18, 2);
BEGIN
    -- Start a transaction
    BEGIN

    -- Retrieve a WithdrawOutbox entry with TransactionHistoryStatusId=0 and set it to 1
    UPDATE "WithdrawOutbox"
    SET "TransactionHistoryStatusId" = 1,"TransactionHistoryStatusChangeDate"=CURRENT_TIMESTAMP
    WHERE "Id" = (
        SELECT "Id" FROM "WithdrawOutbox"
        WHERE "TransactionHistoryStatusId" = 0
        FOR UPDATE SKIP LOCKED
        LIMIT 1
    )
    RETURNING "Id", "TransactionGuid", "AccountId", "Amount", "CreatedDate", "RunningBalance"
    INTO id, transaction_guid, account_id, amount, created_date, running_balance;

	IF id IS NULL THEN
		return;
	END IF;
    -- Check if a row was updated
    
	-- Insert the retrieved values into the TransactionHistory table
	INSERT INTO "TransactionHistory" ("TransactionGuid", "AccountId", "Amount", "TransactionDate", "RunningBalance")
	VALUES (transaction_guid, account_id, amount, created_date, running_balance);

	-- Update the TransactionHistoryStatusId to 2
	UPDATE "WithdrawOutbox"
	SET "TransactionHistoryStatusId" = 2,"TransactionHistoryStatusChangeDate"=CURRENT_TIMESTAMP
	WHERE "Id" = id;

EXCEPTION
    WHEN others THEN
        -- Rollback the transaction on any exception
        ROLLBACK;
		result_message:=SQLERRM;
        RAISE NOTICE 'An error occurred: %', SQLERRM;
	END;
	COMMIT;
END;
$BODY$;
ALTER PROCEDURE public."InsertTransactionHistoryFromWithdrawOutbox"()
    OWNER TO postgres;
	
-- PROCEDURE: public.Withdraw(bigint, numeric, uuid)

-- DROP PROCEDURE IF EXISTS public."Withdraw"(bigint, numeric, uuid);

-- PROCEDURE: public.Withdraw(bigint, numeric, uuid)

-- DROP PROCEDURE IF EXISTS public."Withdraw"(bigint, numeric, uuid);

CREATE OR REPLACE PROCEDURE public."Withdraw"(
	IN accountid bigint,
	IN amount numeric,
	IN transactionguid uuid,
	OUT balance numeric,
	OUT withdrawalstatusid integer,
	OUT errormessage text)
LANGUAGE 'plpgsql'
AS $BODY$
DECLARE
    rows_affected INT;  -- Variable to store the number of rows affected by UPDATE
BEGIN
    -- Start a transaction
    BEGIN

    -- Deduct the amount from the account balance
    UPDATE "AccountBalance"
    SET "Balance" = "Balance" - amount
    WHERE "AccountId" = accountId
      AND "Balance" >= amount
    RETURNING "Balance" INTO balance;

    -- Get the number of rows affected by the update
    GET DIAGNOSTICS rows_affected = ROW_COUNT;	

    IF rows_affected = 0 THEN
        WithdrawalStatusId := 1;  -- Insufficient balance status
        RETURN;
    END IF;

	
    -- Insert a new record into the WithdrawOutbox
    INSERT INTO "WithdrawOutbox" ("TransactionGuid", "AccountId", "Amount","RunningBalance")
    VALUES (transactionGuid, accountId, amount,balance );
	
    -- Commit the transaction
WithdrawalStatusId := 20;
    

      -- Completed successfully status

EXCEPTION
    WHEN others THEN
        -- Rollback the transaction on any exception
        ROLLBACK;
        WithdrawalStatusId := 3;  -- Internal error status
        ErrorMessage := SQLERRM;  -- Capture the error message
	END;
	COMMIT;
END;
$BODY$;
ALTER PROCEDURE public."Withdraw"(bigint, numeric, uuid)
    OWNER TO postgres;
