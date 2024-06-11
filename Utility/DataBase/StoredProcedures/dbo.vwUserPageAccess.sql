ALTER VIEW vwUserPageAccess AS
-- For Direct Pages Data
SELECT 
  DISTINCT 
  u.ProfileId,
  p.PageId,
  p.PageUrl,
  p.PageDescription,
  p.MenuImgPath
FROM tblUserPermission u WITH (NOLOCK) 
INNER JOIN tblPage p WITH (NOLOCK) ON p.PageId = u.PageId
INNER JOIN tblUserProfile tbup WITH (NOLOCK) on tbup.ProfileId = u.ProfileId 
INNER JOIN tblCurrentUser tcu WITH (NOLOCK) on tbup.UserId = tcu.UserId

UNION
-- For Category's Pages Data
SELECT 
  DISTINCT 
  u.ProfileId,
  p.PageId,
  p.PageUrl,
  p.PageDescription,
  p.MenuImgPath
FROM tblUserPermission u WITH (NOLOCK) 
INNER JOIN tblPageCategoryMap pc WITH (NOLOCK) ON pc.PageCatId = u.PageCatId
INNER JOIN tblPage p WITH (NOLOCK) ON p.PageId = pc.PageId
INNER JOIN tblUserProfile tbup WITH (NOLOCK) on tbup.ProfileId = u.ProfileId 
INNER JOIN tblCurrentUser tcu WITH (NOLOCK) on tbup.UserId = tcu.UserId