import { Component , OnInit} from '@angular/core';
import {HttpClient } from '@angular/common/http';
@Component({
  selector: 'app-productslist',
  templateUrl: './productslist.component.html',
  styleUrls: ['./productslist.component.css']
})
export class ProductslistComponent implements OnInit {
  userid: any;
  Products : any[]=[];
  
  filter =this.Products;
  constructor(private http:HttpClient){ }
img : any[]=['img1.jpg','img2.jpg','img3.jpg','img4.jpg','img5.jpg','img6.jpg','img7.jpg']


  ngOnInit(): void {
    this.http.get("https://localhost:44318/api/Products").subscribe((resultData : any)=>
    {
      console.log(resultData);
      this.Products=resultData;
      this.filter=this.Products;
      
    })
  }

  search(event:any){
console.log(event.target.value);
this.filter =this.Products.filter(p => (p.name).includes(event.target.value) )
//this.Products=this.filter;
}

  addCart(productId:any){
    this.userid=localStorage.getItem('userid');
    console.log(this.userid);
    const data = { userId: this.userid, productId: productId };
    this.http.post("https://localhost:44318/api/Users/Cart",data).subscribe((resultData:any)=>{
      console.log(resultData);
      if(resultData.productExists=="yes"){
        alert("Product already exists");
      }
      else{
        alert('cart Item added succesfully');
      }
    },
  (error)=>{
    if(error.status === 400 || error.status === 401){
      alert('Please log in');
    }
  }
  ) 
  }
}
