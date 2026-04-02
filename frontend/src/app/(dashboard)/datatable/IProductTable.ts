export interface IProductTable {
    id: string;
    name: string;
    description?: string;
    price: number;
    categoryId: string;
    imageUrl?: string;
    stock?: number;
    isActive?: boolean;
}