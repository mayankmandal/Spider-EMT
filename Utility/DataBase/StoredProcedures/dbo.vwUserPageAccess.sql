ALTER VIEW vwUserPageAccess AS
-- For Direct Pages Data
SELECT 
  DISTINCT 
  u.ProfileId,
  p.PageId,
  p.PageUrl,
  p.PageDescription,
  tbup.UserId
FROM tblUserPermission u WITH (NOLOCK) 
INNER JOIN tblPage p WITH (NOLOCK) ON p.PageId = u.PageId
INNER JOIN AspNetUserRoles tbup WITH (NOLOCK) on tbup.RoleId = u.ProfileId 

UNION

-- For Category's Pages Data
SELECT 
  DISTINCT 
  u.ProfileId,
  p.PageId,
  p.PageUrl,
  p.PageDescription,
  tbup.UserId
FROM tblUserPermission u WITH (NOLOCK) 
INNER JOIN tblPageCategoryMap pc WITH (NOLOCK) ON pc.PageCatId = u.PageCatId
INNER JOIN tblPage p WITH (NOLOCK) ON p.PageId = pc.PageId
INNER JOIN AspNetUserRoles tbup WITH (NOLOCK) on tbup.RoleId = u.ProfileId 
