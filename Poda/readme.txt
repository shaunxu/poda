PODA - Parallel Orient Data Access

.. Designed and implemented by Shaun Ziyan Xu (jfarrio@gmail.com)
.. Owned by chayu.org (http://www.chayu.org/)

.. STEP 1
   Prepare the application configuration file for PODA. You can refer the sample.app.config in the release package.

.. STEP 2
   Initlize the PODA at the begining of your application started. Use Poda.Factory.Config().

.. STEP 3
   Create the PODA instance by using Poda.Factory.Create() and wrapped in the 'using' block.
   Do not forget to invoke poda.Commit() to save the changes if you insert or update the records.
   If you need to retireve the records in refernece table please use ReferenceOn(tableName) method.
   If you need to insert or update the recrods in reference table please use FederationOnAll() method.