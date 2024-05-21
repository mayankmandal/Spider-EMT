ALTER VIEW vwUserPageAccess AS

SELECT 
  DISTINCT 
  u.ProfileId,
  p.PageId,
  p.PageUrl,
  p.PageDescription,
  p.MenuImgPath
FROM tblUserPermission u 
INNER JOIN tblPage p ON p.PageId = u.PageId
INNER JOIN tblUserProfile tbup on tbup.ProfileId = u.ProfileId 
INNER JOIN tblCurrentUser tcu on tbup.UserId = tcu.UserId

UNION

SELECT 
  DISTINCT 
  u.ProfileId,
  p.PageId,
  p.PageUrl,
  p.PageDescription,
  p.MenuImgPath
FROM tblUserPermission u
INNER JOIN tblPageCategoryMap pc ON pc.PageCatId = u.PageCatId
INNER JOIN tblPage p ON p.PageId = pc.PageId
INNER JOIN tblUserProfile tbup on tbup.ProfileId = u.ProfileId 
INNER JOIN tblCurrentUser tcu on tbup.UserId = tcu.UserId