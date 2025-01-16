# Transactions.Tests

This is an example project that shows how to handle transaction with EF Core.

Key points:

* create transaction withing a using block
    * choose the isolation level that best fits need
    * SaveChanges maybe called multiple times
    * only call Commit() on the transaction once all work is done
        this will create an atomic transaction for all work and all work will be kept or all work will be rolled back
    * rollback the transaction if an exception is thrown
    * rollback the transaction if something is not valid
* when RetryOnFailure is enabled, a new execution strategy will need to be created
    * the new execution strategy will not adopy the default strategy options, like retry on failure

