import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
@Component({
  selector: 'app-signup',
  templateUrl: './signup.component.html',
  styleUrls: ['./signup.component.css']
})
export class SignupComponent { 
signup = new FormGroup({
  firstname: new FormControl('',[Validators.required,Validators.minLength(3),Validators.pattern('[a-zA-Z]*')]),
  lastname:  new FormControl('',[Validators.required,Validators.minLength(3),Validators.pattern('[a-zA-Z]*')]),
  email:new FormControl('',[Validators.required,Validators.email]),
  password: new FormControl('',[Validators.required,Validators.maxLength(15),Validators.pattern('^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*[^A-Za-z0-9]).{6,}$')]),
  phonenumber:new FormControl('',[Validators.required,Validators.pattern('[0-9]{10}')]),
  address:new FormControl('',[Validators.required])
})
 
constructor(private http:HttpClient,private router:Router){} 

onSignUp(){
  this.http.post("https://localhost:44318/api/Users",this.signup.value).subscribe((resultdata:any) =>{
    console.log(resultdata);
    alert('user added succesfully');
    this.router.navigateByUrl('login');
   }, (error)=> {
    console.log(error);
    if(error.status === 400){
      alert('user already exists');
    }
   })

   
}
}
 