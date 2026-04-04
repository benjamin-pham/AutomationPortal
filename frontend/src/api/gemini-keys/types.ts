export interface GeminiKeyListItem {
  id: string;
  name: string;
  maskedKey: string;
  assignedUserId: string;
  assignedUsername: string;
  createdAt: string;
}

export interface GeminiKeyPagedResponse {
  items: GeminiKeyListItem[];
  pageNumber: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface CreateGeminiKeyRequest {
  name: string;
  keyValue: string;
  userId: string;
  replaceExisting: boolean;
}

export interface UpdateGeminiKeyRequest {
  name: string;
  keyValue: string;
  userId: string;
  replaceExisting: boolean;
}

export interface CreateGeminiKeyResponse {
  id: string;
  name: string;
}
