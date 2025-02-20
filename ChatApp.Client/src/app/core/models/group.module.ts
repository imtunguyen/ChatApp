export interface Group {
  id: number;
  name: string;
  creator: string;
  createdAt: Date;
  updatedAt: Date;
}

export interface AddGroup {
  name: string;
  creator: string;
  users: string[];
}
