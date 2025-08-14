import { Component } from '@angular/core';

interface TimelineItem {
  year: string;
  description: string;
}

interface TeamMember {
  name: string;
  position: string;
  image: string;
}

interface Feature {
  title: string;
  icon: string;
  description: string;
}

@Component({
  selector: 'app-about',
  templateUrl: './about.component.html',
  styleUrls: ['./about.component.scss']
})
export class AboutComponent {
  // Company statistics
  stats = {
    revenue: '$12,5M',
    revenueDescription: 'total revenue from\n2001 - 2023',
    orders: '12K+',
    ordersDescription: 'orders delivered\nsuccessful on everyday',
    stores: '725+',
    storesDescription: 'store and office in U.S\nand worldwide'
  };

  // Company features
  features: Feature[] = [
    {
      title: '100% authentic\nproducts',
      icon: 'fas fa-shield-alt',
      description: 'Swoo Tech Mart just distribute 100% authorized products &\nguarantee quality. Nulla porta nulla nec orci vulputate, id\nrutrum sapien varius.'
    },
    {
      title: 'fast\ndelivery',
      icon: 'fas fa-shipping-fast',
      description: 'Fast shipping with a lots of option to delivery. 100%\nguarantee that your goods alway on time and perserve\nquality.'
    },
    {
      title: 'affordable\nprice',
      icon: 'fas fa-tags',
      description: 'We offer an affordable & competitive price with a lots of\nspecial promotions.'
    }
  ];

  // Company timeline - left column
  timelineLeft: TimelineItem[] = [
    { year: '1997:', description: 'A small store located in Brooklyn Town, USA' },
    { year: '1998:', description: 'It is a long established fact that a reader will be distracted by the readable' },
    { year: '2000:', description: 'Lorem Ipsum is simply dummy text of the printing and typesetting industry' },
    { year: '2002:', description: 'Lorem Ipsum has been the industry\'s standard dummy text ever since the' },
    { year: '2004:', description: 'Contrary to popular belief, Lorem Ipsum is not simply random text' },
    { year: '2005:', description: 'The point of using Lorem Ipsum is that it has a more-or-less normal distribution of letters, as opposed to using \'Content here' },
    { year: '2006:', description: 'There are many variations of passages of Lorem Ipsum available, but the majority have suffered alteration in some form, by injected humour, or randomised words which don\'t look even slightly believable.' },
    { year: '2010:', description: 'All the Lorem Ipsum generators on the Internet tend to repeat predefined' },
    { year: '2013:', description: 'Lorem Ipsum comes from sections 1.10.32' }
  ];

  // Company timeline - right column
  timelineRight: TimelineItem[] = [
    { year: '2014:', description: 'There are many variations of passages of Lorem Ipsum available, but the majority have suffered alteration in some form' },
    { year: '2016:', description: 'All the Lorem Ipsum generators on the Internet tend to repeat predefined chunks as necessary' },
    { year: '2020:', description: 'Lorem Ipsum comes from sections 1.10.32' },
    { year: '2021:', description: 'Making this the first true generator on the Internet' },
    { year: '2022:', description: 'Lorem Ipsum which looks reasonable. The generated Lorem Ipsum is therefore always free from repetition, injected humour' },
    { year: '2023:', description: 'here are many variations of passages of Lorem Ipsum available, but the majority have suffered alteration in some form' }
  ];

  // Leadership team
  teamMembers: TeamMember[] = [
    {
      name: 'Henry Avery',
      position: 'Chairman',
      image: 'https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=250&h=320&fit=crop'
    },
    {
      name: 'Michael Edward',
      position: 'Vice President',
      image: 'https://images.unsplash.com/photo-1500648767791-00dcc994a43e?w=250&h=320&fit=crop'
    },
    {
      name: 'Eden Hazard',
      position: 'CEO',
      image: 'https://images.unsplash.com/photo-1472099645785-5658abf4ff4e?w=250&h=320&fit=crop'
    },
    {
      name: 'Robert Downey Jr',
      position: 'CEO',
      image: 'https://images.unsplash.com/photo-1560250097-0b93528c311a?w=250&h=320&fit=crop'
    },
    {
      name: 'Nathan Drake',
      position: 'strategist director',
      image: 'https://images.unsplash.com/photo-1519085360753-af0119f7cbe7?w=250&h=320&fit=crop'
    }
  ];
}
