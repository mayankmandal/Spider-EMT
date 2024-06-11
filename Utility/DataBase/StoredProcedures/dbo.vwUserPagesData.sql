CREATE VIEW vwUserPagesData AS
WITH CombinedData AS (
  -- For Pages Data
  SELECT 
    DISTINCT 
    u.ProfileId,
    p.PageId,
    p.PageUrl,
    p.PageDescription,
    p.MenuImgPath,
    u.PageCatId,
    CatagoryName = NULL
  FROM tblUserPermission u WITH (NOLOCK) 
  INNER JOIN tblPage p WITH (NOLOCK) ON p.PageId = u.PageId
  INNER JOIN tblUserProfile tbup WITH (NOLOCK) ON tbup.ProfileId = u.ProfileId 
  INNER JOIN tblCurrentUser tcu WITH (NOLOCK) ON tbup.UserId = tcu.UserId
  WHERE u.PageCatId IS NULL

  UNION  

  -- For Category's Pages Data
  SELECT 
    DISTINCT 
    u.ProfileId,
    p.PageId,
    p.PageUrl,
    p.PageDescription,
    p.MenuImgPath,
    pc.PageCatId,
    tpc.CatagoryName
  FROM tblUserPermission u WITH (NOLOCK) 
  INNER JOIN tblPageCategoryMap pc WITH (NOLOCK) ON pc.PageCatId = u.PageCatId
  INNER JOIN tblPageCatagory tpc WITH (NOLOCK) ON pc.PageCatId = tpc.PageCatId
  INNER JOIN tblPage p WITH (NOLOCK) ON p.PageId = pc.PageId
  INNER JOIN tblUserProfile tbup WITH (NOLOCK) ON tbup.ProfileId = u.ProfileId 
  INNER JOIN tblCurrentUser tcu WITH (NOLOCK) ON tbup.UserId = tcu.UserId
)

SELECT DISTINCT
  cd.ProfileId,
  cd.PageId,
  cd.PageUrl,
  cd.PageDescription,
  cd.MenuImgPath,
  cd.PageCatId,
  cd.CatagoryName
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
