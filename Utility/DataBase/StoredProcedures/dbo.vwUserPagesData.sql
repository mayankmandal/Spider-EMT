ALTER VIEW vwUserPagesData AS
WITH CombinedData AS (
  -- For Pages Data
  SELECT 
    DISTINCT 
    u.ProfileId,
    p.PageId,
    p.PageUrl,
    p.PageDescription,
    u.PageCatId,
    CatagoryName = NULL,
	tbup.UserId
  FROM tblUserPermission u WITH (NOLOCK) 
  INNER JOIN tblPage p WITH (NOLOCK) ON p.PageId = u.PageId
  INNER JOIN tblUserProfile tbup WITH (NOLOCK) ON tbup.ProfileId = u.ProfileId 
  WHERE u.PageCatId IS NULL

  UNION  

  -- For Category's Pages Data
  SELECT 
    DISTINCT 
    u.ProfileId,
    p.PageId,
    p.PageUrl,
    p.PageDescription,
    pc.PageCatId,
    tpc.CatagoryName,
	tbup.UserId
  FROM tblUserPermission u WITH (NOLOCK) 
  INNER JOIN tblPageCategoryMap pc WITH (NOLOCK) ON pc.PageCatId = u.PageCatId
  INNER JOIN tblPageCatagory tpc WITH (NOLOCK) ON pc.PageCatId = tpc.PageCatId
  INNER JOIN tblPage p WITH (NOLOCK) ON p.PageId = pc.PageId
  INNER JOIN tblUserProfile tbup WITH (NOLOCK) ON tbup.ProfileId = u.ProfileId 
)

SELECT DISTINCT
  cd.ProfileId,
  cd.PageId,
  cd.PageUrl,
  cd.PageDescription,
  cd.PageCatId,
  cd.CatagoryName,
  cd.UserId
FROM CombinedData cd
WHERE NOT EXISTS (
  SELECT 1
  FROM CombinedData sub
  WHERE cd.PageId = sub.PageId
    AND sub.PageCatId IS NOT NULL
    AND sub.CatagoryName IS NOT NULL
)
OR
(
  cd.PageCatId IS NOT NULL
  AND cd.CatagoryName IS NOT NULL
);
