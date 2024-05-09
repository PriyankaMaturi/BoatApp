import { HttpClient ,HttpHeaders} from '@angular/common/http';
import { Component, OnInit} from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';
@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

 login = new FormGroup({ 
  uname:new FormControl('',[Validators.required,Validators.email]),
password: new FormControl()
 }) 
 
 
constructor(private http:HttpClient, private router: Router) {}

ngOnInit(): void {
  const token=localStorage.getItem('token');
 const data={ Token : token};
 const httpOptions = {
  headers: new HttpHeaders({'Content-Type': 'application/json'})
} 
  if(token != null){
    this.http.post("https://localhost:44318/api/Users/validateToken", data,httpOptions).subscribe(()=>{
      this.router.navigateByUrl('');
    },
    (error)=>{
      console.log(error);
      if(error.status === 400){
        alert('Your session has expired or you are not logged in. Please login again');
      }
    }
  )
  }
}

onLogin(){
  //console.log(this.login.controls.uname.value, this.login.controls.password.value)

 this.http.post("https://localhost:44318/api/Users/login",this.login.value).subscribe((resultdata:any) =>{
  console.log(resultdata);
  alert('Login succesful');
 localStorage.setItem("userid",resultdata.id);
 localStorage.setItem("token",resultdata.token);
 this.router.navigateByUrl('/');
 } , 
 (error)=>{
  console.log(error);
  if(error.status === 404){
    alert('user not found');
  }
  if(error.status === 400){
    alert('Wrong Password');
  }
 })  
} 
 }
