import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-discussions',
  templateUrl: './discussions.component.html',
  styleUrls: ['./discussions.component.css']
})
export class DiscussionsComponent implements OnInit {
  discussions: any;

  constructor(private http: HttpClient) { }

  ngOnInit() {
    this.getDiscussions();
  }

  getDiscussions() {
    this.http.get('https://localhost:5001/api/discussion').subscribe(response => {
      this.discussions = response;
    }, error => {
      console.log(error);
    });
  }
}
