INSERT INTO Post (
  Guid,
  LegacyID,
  Name,
  Data,
  IconFileName,
  VisibleFlag,
  UniqueName,
  CreatedUTC,
  ModifiedUTC
) VALUES (
  @Guid,
  @LegacyID,
  @Name,
  @Data,
  @IconFileName,
  @VisibleFlag,
  @UniqueName,
  @CreatedUTC,
  @ModifiedUTC
);