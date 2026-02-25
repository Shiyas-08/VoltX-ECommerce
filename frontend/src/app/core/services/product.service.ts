
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private api = environment.apiUrl + '/products';

  constructor(private http: HttpClient) {}

  //user side 

  getUserProducts() {
    return this.http.get<any>(`${this.api}/User`);
  }

  getProductById(id: number) {
    return this.http.get<any>(`${this.api}/${id}`);
  }

  searchProducts(params: any) {
    return this.http.get<any>(`${this.api}/Search`, { params });
  }

  filterProducts(params: any) {
    return this.http.get<any>(`${this.api}/filter`, { params });
  }

  getPagedProducts(pageNumber: number, pageSize: number) {
    return this.http.get<any>(`${this.api}/Page`, {
      params: { pageNumber, pageSize }
    });
  }

  getFeaturedProducts(pageSize: number = 5) {
    return this.http.get<any>(`${this.api}/filter`, {
      params: {
        pageNumber: 1,
        pageSize,
        isFeatured: true
      }
    });
  }

//admin side 
getAdminProducts() {
  return this.http.get<any>(
    `${this.api}/Admin`,
    { withCredentials: true }
  );
}

createProduct(formData: FormData) {
  return this.http.post<any>(
    this.api,
    formData,
    { withCredentials: true }
  );
}

updateProduct(id: number, formData: FormData) {
  return this.http.put<any>(
    `${this.api}/${id}`,
    formData,
    { withCredentials: true }
  );
}

deleteProduct(id: number) {
  return this.http.delete<any>(
    `${this.api}/${id}`,
    { withCredentials: true }
  );
}

toggleProduct(id: number) {
  return this.http.patch<any>(
    `${this.api}/${id}/toggle`,
    {},
    { withCredentials: true }
  );
}

updateStock(productId: number, quantity: number) {
  return this.http.patch<any>(
    `${this.api}/${productId}/stock`,
    { quantity },
    { withCredentials: true }
  );
}


//images 
  addProductImage(productId: number, image: File) {
    const fd = new FormData();
    fd.append('image', image);
    return this.http.post<any>(`${this.api}/${productId}/images`, fd);
  }

  removeProductImage(imageId: number) {
    return this.http.delete<any>(`${this.api}/images/${imageId}`);
  }
}
