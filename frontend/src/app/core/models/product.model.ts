export interface Product {
 id: number;
  name: string;
  price: number;
  description?: string;
  category?: string;
  image?: string;
  rating?: number;
  quantity?:number;
  isFeatured?: boolean; 
  videoUrl?: string;  
}
 
export interface ProductImage {
  id: number;
  imageUrl: string;
}

export interface AdminProduct {
  id: number;
  name: string;
  price: number;
  description: string;
  categoryId: number;
  categoryName: string;
  stock: number;
  isFeatured: boolean;
  isActive: boolean;
  images: ProductImage[];
}

export interface Product {
  id: number;
  name: string;
  price: number;
  description?: string;

  categoryId: number;
  categoryName: string;

  stock: number;
  IsFeatured: boolean;

  imageUrls?: string[];   
}
