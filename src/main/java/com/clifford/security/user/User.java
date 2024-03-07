package com.clifford.security.user;

import jakarta.persistence.Entity;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.Id;
import jakarta.persistence.Table;
import lombok.AllArgsConstructor;
import lombok.Builder;
import lombok.Data;
import lombok.NoArgsConstructor;

@Data // Lombok annotation to create all the getters, setters, equals, hash, and toString methods
@Builder // Lombok annotation to create the builder pattern
@NoArgsConstructor // Lombok annotation to create the no args constructor
@AllArgsConstructor // Lombok annotation to create the all args constructor
@Entity
@Table(name = "user")
public class User    {
    @Id
    @GeneratedValue 
    private   Integer id;
    private String  firstName;
    private String lastName;
    private String email;
    private String password;



}
