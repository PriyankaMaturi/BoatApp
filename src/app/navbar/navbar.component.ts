import { Component } from '@angular/core';
import { Router } from '@angular/router';
@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent {

   display='';
   isLoggedIn=false;
  constructor(private router:Router) {}

  ngOnInit(): void {
    const token=localStorage.getItem('token');
    
    if(token == null){
      this.display='login';
    }
    else{
      this.display='logout';
      this.isLoggedIn=true;
    }
  }

  navigateToProducts(){
  this.router.navigateByUrl('/');
  }

  logout(){
    localStorage.removeItem('token');
    localStorage.removeItem('userid');
    alert('you have logged out');
    this.router.navigateByUrl('signup');
  }
}
 