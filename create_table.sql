CREATE TABLE logout (
    id INT IDENTITY(1,1) PRIMARY KEY,           -- 自動流水號
    logtime DATETIME NOT NULL DEFAULT GETDATE(),-- 紀錄時間

    computername NVARCHAR(100) NOT NULL,        -- 電腦名稱
    ipaddress NVARCHAR(50) NOT NULL,            -- IP 位址

    drive NVARCHAR(10) NOT NULL,                -- 磁碟代號 (C:, D:...)
    volume NVARCHAR(100) NULL,                  -- 磁碟標籤

    freespace BIGINT NOT NULL,                  -- 可用空間 (GB)
    totalspace BIGINT NOT NULL,                 -- 總空間 (GB)
    freespacepercent INT NOT NULL,              -- 可用空間百分比 (%)

    cpu INT NOT NULL,                           -- CPU 使用率 (%)
    ram INT NOT NULL                            -- 記憶體使用率 (%)
);
