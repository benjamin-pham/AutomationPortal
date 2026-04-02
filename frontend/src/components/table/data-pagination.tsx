import { Button } from "@/components/ui/button";
import {
  Pagination,
  PaginationContent,
  PaginationEllipsis,
  PaginationItem,
} from "@/components/ui/pagination"

interface DataPaginationProps {
  pageIndex: number,
  data: {
    totalPages: number;
    totalItems: number;
  };
  isLoading: boolean;
  onPageChange: (page: number) => void;
}

export default function DataPagination({
  pageIndex,
  data,
  isLoading,
  onPageChange
}: DataPaginationProps) {

  const generatePages = () => {
    const pages: (number | string)[] = [];

    if (data.totalPages <= 10) {
      for (let i = 1; i <= data.totalPages; i++) pages.push(i);
      return pages;
    }

    const startPages = [1, 2];
    const endPages = [data.totalPages - 1, data.totalPages];

    const middlePages = [pageIndex - 1, pageIndex, pageIndex + 1].filter(
      (p) => p > 2 && p < data.totalPages - 1
    );

    pages.push(...startPages);

    if (middlePages.length && middlePages[0] > startPages[startPages.length - 1] + 1) {
      pages.push("...");
    }

    pages.push(...middlePages);

    if (middlePages.length && middlePages[middlePages.length - 1] < endPages[0] - 1) {
      pages.push("...");
    } else if (middlePages.length === 0 && data.totalPages > 4) {
      pages.push("...");
    }

    for (const p of endPages) {
      if (!pages.includes(p)) pages.push(p);
    }

    return pages;
  };

  const pages = generatePages();

  if (pages.length === 0) return null;

  return (
    <div className="flex justify-between items-center">
      <div className="text-sm text-gray-600">
        <p>{data.totalItems} kết quả</p>
      </div>
      <div>
        <Pagination>
          <PaginationContent>
            {/* Previous */}
            <PaginationItem>
              <Button
                disabled={isLoading || pageIndex === 1}
                onClick={(e) => {
                  e.preventDefault();
                  onPageChange(pageIndex - 1);
                }}
                className="cursor-pointer h-8 px-2 rounded-md border border-gray-300 bg-white text-gray-700 hover:bg-gray-100 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                Previous
              </Button>
            </PaginationItem>

            {/* Pages */}
            {pages.map((p, idx) => (
              <PaginationItem key={`${p}-${idx}`}>
                {p === "..." ? (
                  <PaginationEllipsis />
                ) : (
                  <Button
                    disabled={isLoading}
                    onClick={(e) => {
                      e.preventDefault();
                      onPageChange(p as number);
                    }}
                    className={`cursor-pointer h-8 min-w-8 px-2 py-0 rounded-md border flex items-center justify-center text-sm font-medium ${p === pageIndex
                      ? "bg-primary text-primary-foreground border-primary"
                      : "bg-white text-gray-700 border-gray-200 hover:bg-gray-100"
                      } disabled:opacity-50 disabled:cursor-not-allowed`}
                  >
                    {p}
                  </Button>
                )}
              </PaginationItem>
            ))}

            {/* Next */}
            <PaginationItem>
              <Button
                disabled={isLoading || pageIndex === data.totalPages}
                onClick={(e) => {
                  e.preventDefault();
                  onPageChange(pageIndex + 1);
                }}
                className="cursor-pointer h-8 px-2 rounded-md border border-gray-300 bg-white text-gray-700 hover:bg-gray-100 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                Next
              </Button>
            </PaginationItem>
          </PaginationContent>
        </Pagination>
      </div>
    </div>

  )
}
