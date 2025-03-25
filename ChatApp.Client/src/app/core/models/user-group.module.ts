export interface UserGroup{
  groupId: number;
  userId: string;
  roleId: string;
  removedAt: Date;
  isRemoved: boolean;
}
export interface UserGroupAdd{
  groupId: number;
  userId: string;
  roleId: string;

}
