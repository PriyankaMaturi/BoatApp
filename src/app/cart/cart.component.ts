import { Component, OnDestroy, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
 @Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.css']
})
export class CartComponent implements OnInit{
 userid: any;
 Products : any[]=[];
 img : any[]=['img1.jpg','img2.jpg','img3.jpg','img4.jpg','img5.jpg','img6.jpg','img7.jpg']


 constructor(private http:HttpClient, private router:Router){

}
ngOnInit(): void {
  this.userid=localStorage.getItem('userid');
  this.http.get(`https://localhost:44318/api/Users/Cart?UserId=${this.userid}`).subscribe((resultData : any)=>{
    console.log(resultData);
      this.Products=resultData;
  },
  (error)=>{
    if(error.status === 401){
      alert('Please log in');
      this.router.navigateByUrl('login');
    }
  }
)
}

 
increaseItem(productId:any){
   this.userid=localStorage.getItem('userid');
  console.log(this.userid);
  const data = { userId: this.userid, productId: productId };
  this.http.post("https://localhost:44318/api/Users/Cart",data).subscribe((resultData:any)=>{
    console.log(resultData);
    
      alert('cart Item added succesfully');
      this.ngOnInit();
     
  },
  (error)=>{
    if(error.status === 400 || error.status === 401){
      alert('Please log in');
      this.router.navigateByUrl('login');
    }
  }
) 

}
decreaseItem(productId:any){
  this.userid=localStorage.getItem('userid');
  const data = { userId: this.userid, productId: productId };
  this.http.post("https://localhost:44318/api/Users/DecreaseProductFromCart",data).subscribe((resultData:any)=>{
    console.log(resultData);
    
     // alert('cart Item removed succesfully');
      this.ngOnInit();
     
  },

  (error)=>{
    if(error.status === 400 || error.status === 401){
      alert('Please log in');
      this.router.navigateByUrl('login');
    }
  }) 
}
deleteCartItem(productId:any){
  this.userid=localStorage.getItem('userid');
  const data = { userId: this.userid, productId: productId };
  this.http.delete("https://localhost:44318/api/Users/deleteCartItem",{ body: data }).subscribe((resultData: any) => {
  alert("Cart Item deleted successfully");
  this.ngOnInit();

},

(error)=>{
  if(error.status === 400 || error.status === 401){
    alert('Please log in');
    this.router.navigateByUrl('login');
  }
});

} 

getTotalPrice(){
  let TotalPrice=0;
for(const productitem of this.Products){
  TotalPrice+=productitem.productCount*productitem.product.price;
}
return TotalPrice;

}
}
