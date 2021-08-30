# dotnet_aws_calculator

I've included the relevant source files (xaml, cs, python).

For the WPF application I used binding on the text label 'Label Display' (the calculator display) to append operands and operators from the buttons as a single string 
(Fastest and most dynamic way I could think of to parse with the below algorithms and a total application package less than 200 lines, 
this also allows me to forgo regex parsing the operators and operands as it can become costly on a server). 
I've utilized a Regex pattern that will keep it dynamic and to check if the expression the user inputs meets can be submitted to the server. 

# Features:
- *Positive, and negative numbers* 
- *Decimals*
- *Error checking on client using Regex*
- *Label binding to value for INotifyPropertyChanged event*


# If this was an actual product I would add the following features, but left them out for time:
- Error handling if the network is down
- Button scaling 
- Handler on submit button to set active if regex condition is met
- Key input
- Styling 
- Turn Label display into it's own class
- Async lambda calls
- On the AWS Lambda side it's written with Python with a public gateway API. I could have gone with the Python `eval()` function, but being able to interpret raw strings could be a security concern on the server. So I decided to extend my own evaluate function when it's invoked the payload (string) is passed into a  shunting-yard algorithm that creates a stack that evaluates the expression as a Reverse Polish notation. This returns a Json object `calculation` that I pass back into my display. The algorithm can be extended to handle more than 4 arithmetic operations but for the practice decided to keep it basic. 
  

For technology I've decided to go with a AWS lambda function written in Python and not a client ran server because in the initial interview:
1. I was asked if I've ever used AWS services, thought this would be more relevant
2. I was also asked my proficiency in Python
3. It's a quick system to invoke and migrate
4. If I needed to I could invoke an `eval()` function if security wasn't a concern and kept development down to 4-5 lines

I'll be shutting down the Lambda function by next weekend (oct 20th) so please let me know if you need more time. If you or the team have any further questions please feel free to email me.