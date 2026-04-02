import { IProductTable } from '@/app/(dashboard)/datatable/IProductTable'
import PageInteractive from '@/app/(dashboard)/datatable/page.interactive'

export default function page() {
    const data: IProductTable[] = [
        {
            id: "1",
            name: 'Product 1',
            price: 10.99,
            categoryId: 'Category A',
            stock: 100,
            description: 'This is a description for Product 1',
        },
        {
            id: "2",
            name: 'Product 1',
            price: 10.99,
            categoryId: 'Category A',
            stock: 100,
            description: 'This is a description for Product 1',
        },
        {
            id: "3",
            name: 'Product 1',
            price: 10.99,
            categoryId: 'Category A',
            stock: 100,
            description: 'This is a description for Product 1',
        },
        {
            id: "4",
            name: 'Product 1',
            price: 10.99,
            categoryId: 'Category A',
            stock: 100,
            description: 'This is a description for Product 1',
        },
        {
            id: "5",
            name: 'Product 1',
            price: 10.99,
            categoryId: 'Category A',
            stock: 100,
            description: 'This is a description for Product 1',
        }
    ]
    return (
        <div>
            <PageInteractive initialData={data} />
        </div>
    )
}
