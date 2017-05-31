INSERT INTO Article (
  Guid,
  LegacyID,
  Name,
  Description,
  Category,
  ContentURL,
  DefaultSection,
  VisibleFlag,
  UniqueName,
  HideButtonsFlag,
  HideDropDownFlag,
  CreatedUTC,
  ModifiedUTC
) VALUES (
  @Guid,
  @LegacyID,
  @Name,
  @Description,
  @Category,
  @ContentURL,
  @DefaultSection,
  @VisibleFlag,
  @UniqueName,
  @HideButtonsFlag,
  @HideDropDownFlag,
  @CreatedUTC,
  @ModifiedUTC
);