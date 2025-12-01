

  INSERT INTO [Iowa].[dbo].[Packages] VALUES
(NEWID(), NEWID(), 'Table A', 'Description for table A', 'https://img.com/a.png', 120000, 'VND', GETDATE(), NULL, NEWID(), NULL),

(NEWID(), NEWID(), 'Table B', 'Description for table B', 'https://img.com/b.png', 250000, 'VND', GETDATE(), GETDATE(), NEWID(), NEWID()),

(NEWID(), NEWID(), 'Table C', 'Description for table C', 'https://img.com/c.png', NULL, 'USD', GETDATE(), NULL, NEWID(), NULL),

(NEWID(), NEWID(), 'VIP Table', 'Premium table with full features', 'https://img.com/vip.png', 499.99, 'USD', GETDATE(), GETDATE(), NEWID(), NEWID()),

(NEWID(), NEWID(), 'Basic Table', 'Entry-level table option', 'https://img.com/basic.png', 9.99, 'USD', GETDATE(), NULL, NEWID(), NULL);

