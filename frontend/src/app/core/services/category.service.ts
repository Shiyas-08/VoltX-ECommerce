import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private api=environment.apiUrl+'/categories'


  constructor(private http:HttpClient) { }


  getAll(){
    return this.http.get<any>(this.api);
  }
  create(name:string){
    return this.http.post<any>(this.api,{name});
  }
  update(id:number,name:string){
    return this.http.put<any>(`${this.api}/{id}`,{name});
  }
  toggle(id:number){
    return this.http.patch<any>(`${this.api}/${id}/toggle`,{});
  }

  getById(id:number){
    return this.http.get<any>(`${this.api}/${id}`);
  }
}
